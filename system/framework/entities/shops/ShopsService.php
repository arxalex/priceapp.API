<?php

namespace framework\entities\shops;

use framework\entities\default_entities\DefaultEntitiesService;

class ShopsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "shops\\Shop";
        $this->tableName = "pa_shops";
    }
    public function getItemFromShop(int $shopId){
        
    }
}
