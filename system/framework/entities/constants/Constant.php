<?php

namespace framework\entities\constants;

use framework\entities\default_entities\DefaultEntity;

class Constant extends DefaultEntity
{
    public ?int $id;
    public ?string $label;
    public ?string $value;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
        ?string $value = null
    ) {
        $this->id = $id;
        $this->label = $label;
        $this->value = $value;
    }
    
}
