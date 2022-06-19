<?php

namespace framework\entities_proxy\items;

use framework\entities_proxy\default_entities\DefaultEntitiesService;

class AtbItemsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "items\\AtbItem";
        $this->tableName = "pa_items_atb";
    }
}
