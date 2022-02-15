<?php

namespace framework\entities\countries;

use framework\entities\default_entities\DefaultEntitiesService;

class CountriesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "countries\\Country";
        $this->tableName = "pa_countries";
    }
}
