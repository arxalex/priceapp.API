<?php

namespace framework\entities\tokens;

use DateTime;
use framework\entities\default_entities\DefaultEntity;

class Token extends DefaultEntity
{
    public ?int $id;
    public ?int $userid;
    public ?string $token;
    public ?int $expires;

    public function __construct(
        ?int $id = null,
        ?int $userid = null,
        ?string $token = null,
        ?int $expires = null
    ) {
        $this->id = $id;
        $this->userid = $userid;
        $this->token = $token;
        $this->expires = $expires;
    }
    
}
