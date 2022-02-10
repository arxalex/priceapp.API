<?php

namespace framework\entities\items;

use framework\entities\default_entities\DefaultEntitiesService;

class ItemsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "items\\Item";
        $this->tableName = "pa_items";
    }
}
