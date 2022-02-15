<?php

namespace framework\entities\countries;

use framework\entities\default_entities\DefaultEntity;

class Country extends DefaultEntity
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
