<?php

namespace framework\shops\silpo;

use framework\database\NumericHelper;
use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\items\Item;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\packages\PackagesService;
use PDO;
use framework\shops\silpo\SilpoItemModel;

class SilpoItemsGetter
{
    private CategoriesService $_categoriesService;
    private CategoriesLinkService $_categoriesLinkService;
    private PackagesService $_packageService;
    private BrandsService $_brandService;

    public function __construct()
    {
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
        $this->_packageService = new PackagesService();
        $this->_brandService = new BrandsService();
    }
    public function get(int $categotyId, int $fillialId = 2043): array
    {
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

        $options = [
            'http' => [
                'header'  => "Content-Type: application/json;charset=UTF-8\r\n",
                'method'  => 'POST',
                'content' => $data
            ]
        ];
        $context  = stream_context_create($options);
        $result = json_decode(file_get_contents($url, false, $context));

        $items = [];
        foreach ($result->items as $value) {
            $package = $this->getShopItemParam($value, 'packageType');
            $units = NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'numberOfUnits'), true);
            if ($this->getShopItemParam($value, 'isWeighted') == "") {
                $package = "на вагу";
            }
            if (substr($value->units, -2) == "кг") {
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -2));
            } elseif (substr($value->units, -1) == "г") {
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -1)) / 1000;
            } elseif (substr($value->unit, -5) == "шт/уп") {
                $package = "упаковка";
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -5), true);
            }
            $items[] = new SilpoItemModel(
                $value->id,
                $value->name,
                $value->mainImage,
                (($value->categories)[count($value->categories) - 1])->id,
                $this->getShopItemParam($value, 'trademark'),
                $package,
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'alcoholContent'), true),
                $units,
                NumericHelper::toFloatOrNull($this->calorieConverter($this->getShopItemParam($value, 'calorie')), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'carbohydrates'), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'fats'), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'proteins'), true),
                $this->getShopItemParam($value, 'country')
            );
        }
        return $items;
    }
    private function getShopItemParam($item, string $param, bool $nullOrFalse = true)
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
            if ($nullOrFalse) {
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
    public function convertFromSilpoToCommonModel(SilpoItemModel $silpoItem): Item
    {
        $baseCategory = $this->_categoriesLinkService->getItemsFromDB([
            'categoryshopid' => [$silpoItem->shopcategoryid]
        ]);
        $category = $this->_categoriesService->getCategoryByName($silpoItem->label, $baseCategory[0]);
        $brand = $this->_brandService->getBrand($silpoItem->brand);
        $package = $this->_packageService->getPackage($silpoItem->label);
        $commonItem = new Item(
            null,
            $silpoItem->label,
            $silpoItem->image,
            $category->id,
            $brand->id,
            $package->id,
            $silpoItem->units,
            null,
            null,
            null,
            $silpoItem->calorie,
            $silpoItem->carbohydrates,
            $silpoItem->fat,
            $silpoItem->proteins,
            [
                'country' => $silpoItem->country,
                'alcohol' => $silpoItem->alcohol
            ]
        );

        return $commonItem;
    }
    private function calorieConverter(?string $cal)
    {
        return $cal != null ? str_replace(",", ".", explode("/", $cal, 2)[0]) : null;
    }
}
