<?php

namespace framework\shops\atb;

use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\entities_proxy\categories\AtbCategoriesService;
use framework\entities_proxy\items\AtbItemsService;
use framework\entities_proxy\prices\PricesService;

class AtbPricesGetter
{   
    private PricesService $_pricesService;
    private AtbItemsService $_atbItemsService;
    private AtbCategoriesService $_atbCategoriesService;

    public function __construct()
    {
        $this->_pricesService = new PricesService();
        $this->_atbItemsService = new AtbItemsService();
        $this->_atbCategoriesService = new AtbCategoriesService();
    }
    public function getItemPriceAndQuantity(int $inshopid, int $atbFilialId = 1) : PriceAndQuantityAtb
    {
        $result = $this->_pricesService->getItemsFromDB(['itemid' => [$inshopid], 'filialid' => [$atbFilialId], 'shopid' => [3]]);

        if(count($result) > 0){
            $priceAndQuantity = new PriceAndQuantityAtb(NumericHelper::toFloat($result[0]->price), NumericHelper::toFloat($result[0]->quantity), $atbFilialId);
        } else {
            $priceAndQuantity = new PriceAndQuantityAtb(0, 0, $atbFilialId);
        }

        return $priceAndQuantity;
    }
    public function getPricesAndQuantitiesByCategory(int $inshopcategoryid, int $atbFilialId = 1) : array
    {
        $subcategories = $this->_atbCategoriesService->getItemsFromDB(['parent' => [$inshopcategoryid]]);
        $splitCategories = array_merge(ListHelper::getColumn($subcategories, 'id'), [$inshopcategoryid]);
        $itemsOfCategory = $this->_atbItemsService->getItemsFromDB(['category' => $splitCategories]);
        $items = $this->_pricesService->getItemsFromDB(['itemid' => ListHelper::getColumn($itemsOfCategory, 'id'), 'filialid' => [$atbFilialId]]);

        $result = [];
        foreach($items as $item){
            $priceAndQuantity = new PriceAndQuantityAtb(NumericHelper::toInt($item->itemid), NumericHelper::toFloat($item->price), NumericHelper::toFloat($item->quantity));
            $result[] = $priceAndQuantity;
        }

        return $result;
    }
}
class PriceAndQuantityAtb{
    public int $inshopid;
    public float $price;
    public float $quantity;

    public function __construct(
        int $inshopid,
        float $price,
        float $quantity
    )
    {
        $this->inshopid = $inshopid;
        $this->price = $price;
        $this->quantity = $quantity;
    }
}
