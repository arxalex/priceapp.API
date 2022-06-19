<?php

namespace framework\entities\categories;

use framework\entities\default_entities\DefaultEntitiesService;

class AtbCategoriesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "categories\\AtbCategory";
        $this->tableName = "pa_categories_atb";
    }
}
