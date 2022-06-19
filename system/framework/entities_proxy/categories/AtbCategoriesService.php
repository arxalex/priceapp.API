<?php

namespace framework\entities_proxy\categories;

use framework\entities_proxy\default_entities\DefaultEntitiesService;

class AtbCategoriesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "categories\\AtbCategory";
        $this->tableName = "pa_categories_atb";
    }
}
