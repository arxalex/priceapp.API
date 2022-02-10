<?php

namespace framework\entities\prices;

use framework\database\Request;
use framework\entities\prices\Price;
use framework\database\SqlHelper;

class PricesService
{
    public function __construct()
    {
    }
    public function getPriceFromDB(int $id) {
        $query = "select top 1 * from pa_prices where id = $id";
        $response = (new Request($query))->fetchObject("Price");
    }
    public function insertPriceToDB(Price $price){
        $query = "insert into pa_prices
        values " . SqlHelper::insertObjects([$price]);
        $response = (new Request($query))->execute();
    }
}
