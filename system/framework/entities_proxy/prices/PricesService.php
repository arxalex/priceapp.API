<?php

namespace framework\entities_proxy\prices;

use framework\entities_proxy\default_entities\DefaultEntitiesService;

class PricesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "prices\\Price";
        $this->tableName = "pa_prices";
    }
}
