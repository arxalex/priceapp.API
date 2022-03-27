<?php

namespace endpoint\prices;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\entities\categories\CategoriesService;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\filials\FilialsService;
use framework\entities\items\ItemsService;
use framework\entities\items_link\ItemsLinkService;
use framework\entities\prices\PricesService;
use framework\entities\prices_history\PricesHistoryService;
use framework\shops\silpo\SilpoPricesGetter;
use framework\entities\prices\Price;
use framework\entities\prices_history\PriceHistory;
use framework\shops\silpo\PriceAndQuantitySilpo;
use stdClass;

class UpdatePrices extends BaseEndpointBuilder
{
    private PricesService $_pricesService;
    private ItemsLinkService $_itemsLinkService;
    private PricesHistoryService $_pricesHistoryService;
    private SilpoPricesGetter $_silpoPricesGetter;
    private FilialsService $_filialsService;
    private ItemsService $_itemsService;
    private CategoriesService $_categoriesService;
    private CategoriesLinkService $_categoriesLinkService;
    public function __construct()
    {
        $this->_pricesService = new PricesService();
        $this->_itemsLinkService = new ItemsLinkService();
        $this->_pricesHistoryService = new PricesHistoryService();
        $this->_silpoPricesGetter = new SilpoPricesGetter();
        $this->_filialsService = new FilialsService();
        $this->_itemsService = new ItemsService();
        $this->_categoriesService = new CategoriesService();
        $this->_categoriesLinkService = new CategoriesLinkService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => [],
            'from' => 1,
            'to' => 20
        ];
    }
    public function build()
    {
        $from = $this->getParam('from');
        $to = $this->getParam('to');
        error_log(set_time_limit(1200));
        $result = new stdClass();
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));

        $dateToday = date("Y-m-d");
        $shops = [1];
        $map = [];
        foreach ($shops as $shop) {
            $preMap = [];
            $categoryIds = [];
            $itemsLinks = $this->_itemsLinkService->getItemsFromDB(['shopid' => [$shop]]);
            foreach ($itemsLinks as $item) {
                $categoryid = $this->_itemsService->getItemFromDB($item->itemid)->category;
                $category = $this->_categoriesService->getItemFromDB($categoryid);
                while ($category->parent != null) {
                    $category = $this->_categoriesService->getItemFromDB($category->parent);
                }
                $preMap[] = [
                    'item' => $item,
                    'baseCategory' => $category->id
                ];
                if (!in_array($category->id, $categoryIds)) {
                    $categoryIds[] = $category->id;
                }
            }

            $sortedPreMap = [];

            foreach ($categoryIds as $categoryId) {
                $preSortedPreMap = [];
                foreach ($preMap as $preMapValue) {
                    if ($preMapValue['baseCategory'] == $categoryId) {
                        $preSortedPreMap[] = $preMapValue['item'];
                    }
                }
                $sortedPreMap[] = [
                    'categoryid' => $categoryId,
                    'items' => $preSortedPreMap
                ];
            }

            $map[$shop] = $sortedPreMap;
        }
        $ids = [];
        for ($i = $from; $i <= $to; $i++) {
            $ids[] = $i;
        }
        $filials = $this->_filialsService->getItemsFromDB(["id" => $ids]);
        if (empty($filials)) {
            $result->statusUpdate = false;

            return $result;
        }

        foreach ($filials as $filial) {
            $prices = $this->_pricesService->getItemsFromDB(['filialid' => [$filial->id]]);
            $pricesHistory = $this->_pricesHistoryService->getItemsFromDB(['date' => [$dateToday], 'filialid' => [$filial->id]]);
            $PAQs = [];
            foreach ($itemsLinks as $item) {
                if (ListHelper::isObjectinArray($item, $pricesHistory, ["itemid", "shopid"])) {
                    continue;
                }

                $price = null;

                if ($item->shopid == 1) {
                    $baseCategoryId = $this->getBaseCategoryFromMap($item, $map[$item->shopid]);
                    if ($PAQs[$baseCategoryId] == null) {
                        $PAQs[$baseCategoryId] = $this->_silpoPricesGetter
                            ->getPricesAndQuantitiesByCategory(
                                $this->_categoriesLinkService->getItemsFromDB([
                                    'categoryid' => [$baseCategoryId]
                                ])[0]->categoryshopid,
                                $filial->inshopid
                            );
                    }
                    $PAQObject = $this->getPAQObject($item->inshopid, $PAQs[$baseCategoryId]);
                    $price = $PAQObject->price * NumericHelper::toFloat($item->pricefactor);
                    $quantity = $PAQObject->quantity / NumericHelper::toFloat($item->pricefactor);
                }
                

                if (ListHelper::isObjectinArray($item, $prices, ["itemid", "shopid"])) {
                    $priceObject = ($this->_pricesService->getItemsFromDB([
                        "itemid" => [$item->itemid],
                        "shopid" => [$item->shopid],
                        "filialid" => [$filial->id]
                    ]))[0];
                    $priceObject->price = $price;
                    $priceObject->quantity = $quantity;
                    $this->_pricesService->updateItemInDB($priceObject);
                } else {
                    $this->_pricesService->insertItemToDB(new Price(null, $item->itemid, $item->shopid, $price, $filial->id, $quantity));
                }

                $this->_pricesHistoryService->insertItemToDB(new PriceHistory(null, $item->itemid, $item->shopid, $price, $dateToday, $filial->id));
            }
        }

        $result->statusUpdate = true;

        return $result;
    }
    private function getBaseCategoryFromMap(object $item, array $map): int
    {
        foreach ($map as $value) {
            if (ListHelper::isObjectinArray($item, $value['items'], ['id'])) {
                return $value['categoryid'];
            }
        }
        return -1;
    }
    private function getPAQObject(int $inshopid, array $PAQs): PriceAndQuantitySilpo
    {
        foreach ($PAQs as $value) {
            if ($value->inshopid == $inshopid) {
                return $value;
            }
        }
        return new PriceAndQuantitySilpo($inshopid, 0, 0);
    }
}
