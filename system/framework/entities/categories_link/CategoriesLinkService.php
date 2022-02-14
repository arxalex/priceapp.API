<?php

namespace framework\entities\categories_link;

use framework\entities\default_entities\DefaultEntitiesService;

class CategoriesLinkService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "categories_link\\CategoryLink";
        $this->tableName = "pa_categories_link";
    }
}
