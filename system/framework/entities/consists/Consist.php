<?php

namespace framework\entities\consists;

use framework\entities\default_entities\DefaultEntity;

class Consist extends DefaultEntity
{
    public ?int $id;
    public ?string $label;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
    ) {
        $this->id = $id;
        $this->label = $label;
    }
    
}
