<?php

namespace framework\entities\categories;

class Category
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
