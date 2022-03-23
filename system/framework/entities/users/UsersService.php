<?php

namespace framework\entities\users;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class UsersService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "users\\User";
        $this->tableName = "pa_users";
    }
}
