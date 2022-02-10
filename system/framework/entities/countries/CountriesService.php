<?php

namespace framework\entities\countries;

use framework\database\Request;
use framework\entities\countries\Country;
use framework\database\SqlHelper;

class CountriesService
{
    public function __construct()
    {
    }
    public function getCountryFromDB(int $id) {
        $query = "select top 1 * from pa_countries where id = $id";
        $response = (new Request($query))->fetchObject("Country");
    }
    public function insertCountryToDB(Country $country){
        $query = "insert into pa_countries
        values " . SqlHelper::insertObjects([$country]);
        $response = (new Request($query))->execute();
    }
}
