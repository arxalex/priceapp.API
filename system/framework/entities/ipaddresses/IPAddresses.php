<?php

namespace framework\entities\ipaddresses;

use framework\entities\default_entities\DefaultEntity;

class IPAddresses extends DefaultEntity
{
    public ?int $id;
    public ?string $ipaddresses;
    public ?bool $blacklisted;
    public ?int $userid;

    public function __construct(
        ?int $id = null,
        ?string $ipaddresses = null,
        ?bool $blacklisted = null,
        ?int $userid = null
    ) {
        $this->id = $id;
        $this->ipaddresses = $ipaddresses;
        $this->blacklisted = $blacklisted;
        $this->userid = $userid;
    }
    
}
