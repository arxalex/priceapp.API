<?php

namespace framework\entities\brands;

use framework\entities\default_entities\DefaultEntitiesService;

class BrandsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "brands\\Brand";
        $this->tableName = "pa_brand";
    }
}
