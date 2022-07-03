<?php

namespace framework\entities\brand_alerts;

use framework\entities\default_entities\DefaultEntitiesService;

class BrandAlertsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "brand_alerts\\BrandAlert";
        $this->tableName = "pa_brand_alerts";
    }
}
