<?php

namespace endpoint\prices;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\items\ItemsService;
use framework\entities\prices\PricesService;
use stdClass;

class AnalizePrices extends BaseEndpointBuilder
{
    private PricesService $_pricesService;
    private ItemsService $_itemsService;
    public function __construct()
    {
        $this->_pricesService = new PricesService();
        $this->_itemsService = new ItemsService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => [],
            'method' => 'fix10x'
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        $result = new stdClass();
        $start = time();
        if($this->getParam('method') == 'fix10x'){
            $items = $this->_itemsService->getItemsFromDB();
            foreach($items as $item){
                $prices = $this->_pricesService->getItemsFromDB(['itemid' => [$item->id]]);
                if(count($prices) < 1){
                    continue;
                }
                $priceSum = 0;
                $i = 0;

                foreach($prices as $price){
                    if($price->pricefactor != null) {
                        continue;
                    }
                    $priceSum += $price->price;
                    $i++;
                }
                $priceAvg = $priceSum / $i;

                foreach($prices as $price){
                    if($price->pricefactor != null) {
                        continue;
                    }
                    $factor = $priceAvg / $price->price;
                    if($factor > 6){
                        $price->pricefactor = 10;
                        $this->_pricesService->updateItemInDB($price);
                    } elseif($factor < 0.4) {
                        $price->pricefactor = 0.1;
                        $this->_pricesService->updateItemInDB($price);
                    }
                }
            }
        }

        $result->statusUpdate = true;
        $result->executionTime = time() - $start;

        return $result;
    }
}
