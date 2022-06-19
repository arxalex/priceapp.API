<?php

namespace framework\entities\items;

use framework\entities\default_entities\DefaultEntitiesService;

class AtbItemsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "items\\AtbItem";
        $this->tableName = "pa_items_atb";
    }
}
