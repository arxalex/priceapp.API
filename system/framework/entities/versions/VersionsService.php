<?php

namespace framework\entities\versions;

use framework\entities\default_entities\DefaultEntitiesService;

class VersionsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "versions\\Version";
        $this->tableName = "pa_versions";
    }
}
