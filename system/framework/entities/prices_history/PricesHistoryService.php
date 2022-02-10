<?php

namespace framework\entities\prices_history;

use framework\database\Request;
use framework\entities\prices_history\PriceHistory;
use framework\database\SqlHelper;

class PricesHistoryService
{
    public function __construct()
    {
    }
    public function getPriceHistoryFromDB(int $id) {
        $query = "select top 1 * from pa_prices_history where id = $id";
        $response = (new Request($query))->fetchObject("PriceHistory");
    }
    public function insertPriceHistoryToDB(PriceHistory $priceHistory){
        $query = "insert into pa_prices_history
        values " . SqlHelper::insertObjects([$priceHistory]);
        $response = (new Request($query))->execute();
    }
}
