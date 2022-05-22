<?php

namespace framework\entities\confirm_email;

use framework\entities\default_entities\DefaultEntity;

class ConfirmEmail extends DefaultEntity
{
    public ?int $id;
    public ?int $userid;
    public ?string $token;

    public function __construct(
        ?int $id = null,
        ?int $userid = null,
        ?string $token = null
    ) {
        $this->id = $id;
        $this->userid = $userid;
        $this->token = $token;
    }
    
}
