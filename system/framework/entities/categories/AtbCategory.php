<?php

namespace framework\entities\categories;

use framework\entities\default_entities\DefaultEntity;

class AtbCategory extends DefaultEntity
{
    public ?int $id;
    public ?int $internalid;
    public ?string $label;
    public ?int $parent;

    public function __construct(
        ?int $id = null,
        ?int $internalid = null,
        ?string $label = null,
        ?int $parent = null
    ) {
        $this->id = $id;
        $this->internalid = $internalid;
        $this->label = $label;
        $this->parent = $parent;
    }
    
}
