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
use framework\shops\fora\ForaPricesGetter;
use framework\entities\prices\Price;
use framework\entities\prices_history\PriceHistory;
use framework\shops\atb\AtbPricesGetter;
use framework\shops\silpo\PriceAndQuantitySilpo;
use framework\shops\fora\PriceAndQuantityFora;
use stdClass;

class UpdatePrices extends BaseEndpointBuilder
{
    private PricesService $_pricesService;
    private ItemsLinkService $_itemsLinkService;
    private PricesHistoryService $_pricesHistoryService;
    private SilpoPricesGetter $_silpoPricesGetter;
    private ForaPricesGetter $_foraPricesGetter;
    private AtbPricesGetter $_atbPricesGetter;
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
        $this->_foraPricesGetter = new ForaPricesGetter();
        $this->_atbPricesGetter = new AtbPricesGetter();
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
            'from' => 0,
            'to' => 0
        ];
    }
    public function build()
    {
        $start = time();
        $from = $this->getParam('from');
        $to = $this->getParam('to');
        ini_set('max_execution_time', 0);
        set_time_limit(0);
        $result = new stdClass();
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));

        $dateToday = date("Y-m-d");

        $categoriesFromDB = $this->_categoriesService->getItemsFromDB();
        $categoriesLinksFromDB = $this->_categoriesLinkService->getItemsFromDB();
        $itemsLinksFromDB = $this->_itemsLinkService->getItemsFromDB();
        $itemsFromDB = $this->_itemsService->getItemsFromDB();
        $pricesFromDB = $this->_pricesService->getItemsFromDB();
        $pricesHistoryFromDB = $this->_pricesHistoryService->getItemsFromDB(['date' => [$dateToday]]);

        $shops = [1, 2, 3];
        $map = [];
        foreach ($shops as $shop) {
            $preMap = [];
            $categoryIds = [];
            $itemsLinksInShop = ListHelper::getMultipleByFields($itemsLinksFromDB, ['shopid' => [$shop]]);
            foreach ($itemsLinksInShop as $item) {
                $categoryid = ListHelper::getOneByFields($itemsFromDB, ['id' => $item->itemid])->category;
                $category = ListHelper::getOneByFields($categoriesFromDB, ['id' => $categoryid]);
                while ($category->parent != null) {
                    $category = ListHelper::getOneByFields($categoriesFromDB, ['id' => $category->parent]);
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
        if ($from != 0 && $to != 0) {
            $ids = [];
            for ($i = $from; $i <= $to; $i++) {
                $ids[] = $i;
            }
            $filials = $this->_filialsService->getItemsFromDB(["id" => $ids]);
        } else {
            $filials = $this->_filialsService->getItemsFromDB();
        }
        if (empty($filials)) {
            $result->statusUpdate = false;
            return $result;
        }

        $lastPoint = 0;

        for($i = 0; $i < count($filials); $i++) {
            $pricesHistory = ListHelper::getMultipleByFields($pricesHistoryFromDB, ['filialid' => [$filials[$i]->id]]);

            if(count($pricesHistory) > 0){
                $lastPoint = $i;
            }
        }

        for($i = $lastPoint; $i < count($filials); $i++) {
            $pricesHistory = ListHelper::getMultipleByFields($pricesHistoryFromDB, ['filialid' => [$filials[$i]->id]]);
            $prices = ListHelper::getMultipleByFields($pricesFromDB, ['filialid' => [$filials[$i]->id]]);
            $itemsLinks = ListHelper::getMultipleByFields($itemsLinksFromDB, ['shopid' => [$filials[$i]->shopid]]);

            $PAQs = [];
            foreach ($itemsLinks as $item) {
                if (ListHelper::isObjectinArray($item, $pricesHistory, ["itemid", "shopid"])) {
                    continue;
                }

                $price = null;

                if ($item->shopid == 1) {
                    $baseCategoryId = $this->getBaseCategoryFromMap($item, $map[$item->shopid]);
                    if (!array_key_exists($baseCategoryId, $PAQs) || $PAQs[$baseCategoryId] == null) {
                        $PAQs[$baseCategoryId] = $this->_silpoPricesGetter
                            ->getPricesAndQuantitiesByCategory(
                                ListHelper::getOneByFields($categoriesLinksFromDB, [
                                    'categoryid' => $baseCategoryId,
                                    'shopid' => 1
                                ])->categoryshopid,
                                $filials[$i]->inshopid
                            );
                    }
                    $PAQObject = $this->getPAQObjectSilpo($item->inshopid, $PAQs[$baseCategoryId]);
                    $price = $PAQObject->price * NumericHelper::toFloat($item->pricefactor);
                    $quantity = $PAQObject->quantity / NumericHelper::toFloat($item->pricefactor);
                } elseif ($item->shopid == 2) {
                    $baseCategoryId = $this->getBaseCategoryFromMap($item, $map[$item->shopid]);
                    if (!array_key_exists($baseCategoryId, $PAQs) || $PAQs[$baseCategoryId] == null) {
                        $PAQs[$baseCategoryId] = $this->_foraPricesGetter
                            ->getPricesAndQuantitiesByCategory(
                                ListHelper::getOneByFields($categoriesLinksFromDB, [
                                    'categoryid' => $baseCategoryId,
                                    'shopid' => 2
                                ])->categoryshopid,
                                $filials[$i]->inshopid
                            );
                    }
                    $PAQObject = $this->getPAQObjectFora($item->inshopid, $PAQs[$baseCategoryId]);
                    $price = $PAQObject->price * NumericHelper::toFloat($item->pricefactor);
                    $quantity = $PAQObject->quantity / NumericHelper::toFloat($item->pricefactor);
                } elseif ($item->shopid == 3) {
                    $baseCategoryId = $this->getBaseCategoryFromMap($item, $map[$item->shopid]);
                    if (!array_key_exists($baseCategoryId, $PAQs) || $PAQs[$baseCategoryId] == null) {
                        $PAQs[$baseCategoryId] = $this->_atbPricesGetter
                            ->getPricesAndQuantitiesByCategory(
                                ListHelper::getOneByFields($categoriesLinksFromDB, [
                                    'categoryid' => $baseCategoryId,
                                    'shopid' => 3
                                ])->categoryshopid,
                                $filials[$i]->inshopid
                            );
                    }
                    $PAQObject = $this->getPAQObjectFora($item->inshopid, $PAQs[$baseCategoryId]);
                    $price = $PAQObject->price * NumericHelper::toFloat($item->pricefactor);
                    $quantity = $PAQObject->quantity / NumericHelper::toFloat($item->pricefactor);
                }


                if (ListHelper::isObjectinArray($item, $prices, ["itemid", "shopid"])) {
                    $priceObjects = $this->_pricesService->getItemsFromDB([
                        "itemid" => [$item->itemid],
                        "shopid" => [$item->shopid],
                        "filialid" => [$filials[$i]->id]
                    ]);
                    if(count($priceObjects) > 0){
                        $priceObject =  $priceObjects[0];
                        $priceObject->price = $price;
                        $priceObject->quantity = $quantity;
                        if ($price <= 0 || $quantity <= 0) {
                            $this->_pricesService->deleteItem($priceObject);
                            continue;
                        } else {
                            $this->_pricesService->updateItemInDB($priceObject);
                        }
                    }
                    
                    
                } else {
                    if ($price <= 0 || $quantity <= 0) {
                        continue;
                    } else {
                        $this->_pricesService->insertItemToDB(new Price(null, $item->itemid, $item->shopid, $price, $filials[$i]->id, $quantity));
                    }
                }

                if ($price <= 0 || $quantity <= 0) {
                    continue;
                } else {
                    $this->_pricesHistoryService->insertItemToDB(new PriceHistory(null, $item->itemid, $item->shopid, $price, $dateToday, $filials[$i]->id));
                }
            }
        }

        $result->statusUpdate = true;
        $result->executionTime = time() - $start;

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
    private function getPAQObjectSilpo(int $inshopid, array $PAQs): PriceAndQuantitySilpo
    {
        foreach ($PAQs as $value) {
            if ($value->inshopid == $inshopid) {
                return $value;
            }
        }
        return new PriceAndQuantitySilpo($inshopid, 0, 0);
    }
    private function getPAQObjectFora(int $inshopid, array $PAQs): PriceAndQuantityFora
    {
        foreach ($PAQs as $value) {
            if ($value->inshopid == $inshopid) {
                return $value;
            }
        }
        return new PriceAndQuantityFora($inshopid, 0, 0);
    }
}
