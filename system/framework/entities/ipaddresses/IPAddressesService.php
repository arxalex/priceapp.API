<?php

namespace framework\entities\ipaddresses;

use framework\entities\default_entities\DefaultEntitiesService;

class IPAddressesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "ipaddresses\\IPAddresses";
        $this->tableName = "pa_ipaddresses";
    }
}
