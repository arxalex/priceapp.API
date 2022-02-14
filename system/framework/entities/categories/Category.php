<?php

namespace framework\entities\categories;

use framework\entities\default_entities\DefaultEntity;

class Category extends DefaultEntity
{
    public ?int $id;
    public ?string $label;
    public ?int $parent;
    public ?int $isFilter;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
        ?int $parent = null,
        ?int $isFilter = null
    ) {
        $this->id = $id;
        $this->label = $label;
        $this->parent = $parent;
        $this->isFilter = $isFilter;
    }
    
}
