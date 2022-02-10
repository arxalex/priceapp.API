<?php

namespace framework\entities\packages;

use framework\database\Request;
use framework\entities\packages\Package;
use framework\database\SqlHelper;

class PackagesService
{
    public function __construct()
    {
    }
    public function getPackageFromDB(int $id) {
        $query = "select top 1 * from pa_package where id = $id";
        $response = (new Request($query))->fetchObject("Package");
    }
    public function insertPackageToDB(Package $package){
        $query = "insert into pa_package
        values " . SqlHelper::insertObjects([$package]);
        $response = (new Request($query))->execute();
    }
}
