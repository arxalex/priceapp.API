<?php

namespace framework\shops\silpo;

use framework\database\SqlHelper;
use framework\entities\categories\CategoriesService;
use framework\entities\items\Item;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\categories\Category;
use framework\entities\categories_link\CategoryLink;
use PDO;
use framework\shops\silpo\ItemViewModel;

class ItemsGetter
{
    private CategoriesLinkService $_categoriesLinkService;
    private CategoriesService $_categoriesService;

    public function __construct()
    {
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
    }
    public function get(int $categotyId, int $fillialId = 2043): array
    {
        $shopid = 1;
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'From' => 0,
                'To' => 10000,
                'categoryId' => $categotyId,
                'filialId' => $fillialId,
            ],
            'method' => 'GetSimpleCatalogItems'
        ]);

        $options = array(
            'http' => array(
                'method'  => 'POST',
                'content' => $data
            )
        );
        $context  = stream_context_create($options);
        $result = json_decode(file_get_contents($url, false, $context));
        $items = [];
        $categories = $this->_categoriesService->getItemsFromDB();
        foreach ($result->items as $key => $value) {
            $categoryLink = ($this->_categoriesLinkService->getItemsFromDB(
                [
                    'categoryshopid' => [(($value->categories)[count($value->categories) - 1])->id],
                    'shopid' => $shopid
                ]
            ))[0];
            $category = $this->_categoriesService->getItemFromDB($categoryLink->categoryid);
            $itemTemp = new ItemViewModel(
                null,
                SqlHelper::mysql_escape_mimic($value->name),
                SqlHelper::mysql_escape_mimic($value->mainImage),
                $this->_categoriesService->containsCategory($value->name, $category),
                $this->getShopItemParam($value, 'trademark')
            );
        }
    }
    private function getShopItemParam($item, string $param, bool $falseOrNull = false)
    {
        $isok = false;
        if ($item->parameters) {
            foreach ($item->parameters as $value) {
                if ($value->key == $param) {
                    $isok = true;
                    $tmp = $value->value;
                }
            }
        }
        if ($isok) {
            return $tmp;
        } else {
            if ($falseOrNull) {
                return null;
            } else {
                return false;
            }
        }
    }
    private function getAndInsertParamInTable($value, $param, $table_name, $what, $connect)
    {
        $tmp = get_param($value, $param);
        if ($tmp == false) {
            $tm = 0;
        } else {
            $tmq = "SELECT `id` FROM `$table_name` WHERE `$what` = '" . mysql_escape_mimic($tmp) . "'";
            $statement = $connect->prepare($tmq);
            $statement->execute();
            $tm = $statement->fetch(PDO::FETCH_ASSOC)["id"];
            if ($tm == NULL) {
                $tmiq = "INSERT INTO `$table_name` (`id`, `$what`) VALUES (DEFAULT, '" . mysql_escape_mimic($tmp) . "')";
                $connect->prepare($tmiq)->execute();
                $tm = $connect->lastInsertId();
            }
        }
        return $tm;
    }
}
