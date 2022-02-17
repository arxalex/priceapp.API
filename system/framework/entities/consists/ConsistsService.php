<?php

namespace framework\entities\consists;

use framework\entities\default_entities\DefaultEntitiesService;

class ConsistsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "consists\\Consist";
        $this->tableName = "pa_consists";
    }
}
