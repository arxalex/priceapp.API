<?php

namespace framework\entities\filials;

use framework\entities\default_entities\DefaultEntitiesService;

class FilialsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "filials\\Filial";
        $this->tableName = "pa_filials";
    }
}
