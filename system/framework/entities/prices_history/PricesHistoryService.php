<?php

namespace framework\entities\prices_history;

use framework\entities\default_entities\DefaultEntitiesService;

class PricesHistoryService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "prices_history\\PriceHistory";
        $this->tableName = "pa_prices_history";
    }
}
