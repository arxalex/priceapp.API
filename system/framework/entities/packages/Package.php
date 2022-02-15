<?php

namespace framework\entities\packages;

use framework\entities\default_entities\DefaultEntity;

class Package extends DefaultEntity
{
    public ?int $id;
    public ?string $label;
    public ?string $short;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
        ?string $short = null
    ) {
        $this->id = $id;
        $this->label = $label;
        $this->short = $short;
    }
    
}
