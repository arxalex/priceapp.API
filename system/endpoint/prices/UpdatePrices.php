<?php

namespace endpoint\prices;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\entities\filials\FilialsService;
use framework\entities\items_link\ItemsLinkService;
use framework\entities\prices\PricesService;
use framework\entities\prices_history\PricesHistoryService;
use framework\shops\silpo\SilpoPricesGetter;
use framework\entities\prices\Price;
use framework\entities\prices_history\PriceHistory;
use stdClass;

class UpdatePrices extends BaseEndpointBuilder
{
    private PricesService $_pricesService;
    private ItemsLinkService $_itemsLinkService;
    private PricesHistoryService $_pricesHistoryService;
    private SilpoPricesGetter $_silpoPricesGetter;
    private FilialsService $_filialsService;
    public function __construct()
    {
        $this->_pricesService = new PricesService();
        $this->_itemsLinkService = new ItemsLinkService();
        $this->_pricesHistoryService = new PricesHistoryService();
        $this->_silpoPricesGetter = new SilpoPricesGetter();
        $this->_filialsService = new FilialsService();
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
        $result = new stdClass();
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));

        $dateToday = date("Y-m-d");

        $itemsLinks = $this->_itemsLinkService->getItemsFromDB();

        $i = 0;
        $filials = $this->_filialsService->getItemsFromDB();
        foreach ($filials as $filial) {
            $prices = $this->_pricesService->getItemsFromDB(['filialid' => [$filial->id]]);
            $pricesHistory = $this->_pricesHistoryService->getItemsFromDB(['date' => [$dateToday], 'filialid' => [$filial->id]]);
            foreach ($itemsLinks as $item) {
                if (ListHelper::isObjectinArray($item, $pricesHistory, ["itemid", "shopid"])) {
                    continue;
                }

                if($i > 1000){
                    sleep(120);
                    $i = 0;
                }

                $price = null;

                if ($item->shopid == 1) {
                    $priceAndQuantity = $this->_silpoPricesGetter->getItemPriceAndQuantity($item->inshopid, $filial->inshopid);
                    $price = $priceAndQuantity->price * NumericHelper::toFloat($item->pricefactor);
                    $quantity = $priceAndQuantity->quantity / NumericHelper::toFloat($item->pricefactor);
                    $i++;
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
}
