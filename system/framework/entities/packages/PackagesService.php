<?php

namespace framework\entities\packages;

use framework\database\Request;
use framework\entities\packages\Package;
use framework\database\SqlHelper;
use framework\entities\default_entities\DefaultEntitiesService;
use framework\database\StringHelper;

class PackagesService extends DefaultEntitiesService
{
    public function __construct()
    {        
        $this->className = self::ENTITIES_NAMESPACE . "packages\\Package";
        $this->tableName = "pa_package";
    }
    public function getPackage(?string $str) : Package
    {
        if($str == null)
        {
            return $this->getItemFromDB(0);
        }
        $strArr = StringHelper::nameToKeywords($str);
        $packages = $this->getItemsFromDB([
            'label_like' => $strArr
        ]);
        $rate = StringHelper::rateItemsByKeywords($str, array_column($packages, 'label'));
        return ($this->orderItemsByRate($packages, $rate, 1))[0];
    }
}
