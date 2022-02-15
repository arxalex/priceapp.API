<?php

namespace framework\entities\prices;

use framework\entities\default_entities\DefaultEntitiesService;

class PricesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "prices\\Price";
        $this->tableName = "pa_prices";
    }
}
