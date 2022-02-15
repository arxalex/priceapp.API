<?php

namespace framework\entities\items_link;

use framework\entities\default_entities\DefaultEntitiesService;

class ItemsLinkService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "items_link\\ItemLink";
        $this->tableName = "pa_items_link";
    }
}
