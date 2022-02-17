<?php

namespace framework\shops\silpo;

class SilpoCategoriesModel{
    public ?int $id;
    public ?string $label;

    public function __construct(
        ?int $id = null,
        ?string $label = null
    ) {
        $this->id = $id;
        $this->label = $label;
    }
}