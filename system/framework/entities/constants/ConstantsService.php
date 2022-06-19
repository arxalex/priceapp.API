<?php

namespace framework\entities\constants;

use framework\entities\default_entities\DefaultEntitiesService;

class ConstantsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "constants\\Constant";
        $this->tableName = "pa_constants";
    }
}
