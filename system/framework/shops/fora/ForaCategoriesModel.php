<?php

namespace framework\shops\fora;

class ForaCategoriesModel{
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