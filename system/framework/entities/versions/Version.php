<?php

namespace framework\entities\versions;

use framework\entities\default_entities\DefaultEntity;

class Version extends DefaultEntity
{
    public ?int $id;
    public ?int $version;
    public ?int $major;
    public ?int $minor;
    public ?bool $isminver;

    public function __construct(
        ?int $id = null,
        ?int $version = null,
        ?int $major = null,
        ?int $minor = null,
        ?bool $isminver = null
    ) {
        $this->id = $id;
        $this->version = $version;
        $this->major = $major;
        $this->minor = $minor;
        $this->isminver = $isminver;
    }
    
}
