<?php

namespace framework\entities_proxy\categories;

use framework\entities_proxy\default_entities\DefaultEntity;

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
