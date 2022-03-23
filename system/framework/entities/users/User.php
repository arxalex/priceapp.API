<?php

namespace framework\entities\users;

use framework\entities\default_entities\DefaultEntity;

class User extends DefaultEntity
{
    public ?int $id;
    public ?string $username;
    public ?string $email;
    public ?string $password;

    public function __construct(
        ?int $id = null,
        ?string $username = null,
        ?string $email = null,
        ?string $password = null
    ) {
        $this->id = $id;
        $this->username = $username;
        $this->email = $email;
        $this->password = $password;
    }
    
}
