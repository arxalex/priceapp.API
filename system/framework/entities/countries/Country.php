<?php

namespace framework\entities\countries;

class Country
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
