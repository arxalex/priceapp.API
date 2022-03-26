<?php

namespace endpoint\prices;

use DateTime;
use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\entities\items\ItemsService;
use framework\entities\items_link\ItemsLinkService;
use framework\entities\prices\PricesService;
use framework\entities\prices_history\PricesHistoryService;
use framework\shops\silpo\SilpoItemsGetter;
use framework\entities\prices\Price;
use framework\entities\prices_history\PriceHistory;
use stdClass;

class UpdatePrices extends BaseEndpointBuilder
{
    private PricesService $_pricesService;
    private ItemsService $_itemsService;
    private ItemsLinkService $_itemsLinkService;
    private PricesHistoryService $_pricesHistoryService;
    private SilpoItemsGetter $_silpoItemsGetter;
    public function __construct()
    {
        $this->_pricesService = new PricesService();
        $this->_itemsService = new ItemsService();
        $this->_itemsLinkService = new ItemsLinkService();
        $this->_pricesHistoryService = new PricesHistoryService();
        $this->_silpoItemsGetter = new SilpoItemsGetter();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));

        $dateToday = date("Y-m-d");

        $itemsLinks = $this->_itemsLinkService->getItemsFromDB();
        $prices = $this->_pricesService->getItemsFromDB();
        $pricesHistory = $this->_pricesHistoryService->getItemsFromDB(['date' => [$dateToday]]);

        foreach($itemsLinks as $item){
            if(ListHelper::isObjectinArray($item, $pricesHistory, ["itemid", "shopid"])){
                continue;
            }

            $price = null;

            if($item->shopid == 1){
                $price = $this->_silpoItemsGetter->getItemPrice($item->inshopid) * NumericHelper::toFloat($item->pricefactor);
            }

            if(ListHelper::isObjectinArray($item, $prices, ["itemid", "shopid"])){
                $priceObject = ($this->_pricesService->getItemsFromDB([
                    "itemid" => [$item->itemid],
                    "shopid" => [$item->shopid]
                ]))[0];
                $priceObject->price = $price;
                $this->_pricesService->updateItemInDB($priceObject);
            } else {
                $this->_pricesService->insertItemToDB(new Price(null, $item->itemid, $item->shopid, $price));
            }

            $this->_pricesHistoryService->insertItemToDB(new PriceHistory(null, $item->itemid, $item->shopid, $price, $dateToday));
        }

        return json_encode(["status" => true]);
    }
}
