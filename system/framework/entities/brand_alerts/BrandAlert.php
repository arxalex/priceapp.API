<?php

namespace framework\entities\brand_alerts;

use framework\entities\default_entities\DefaultEntity;

class BrandAlert extends DefaultEntity
{
    public ?int $id;
    public ?int $brandid;
    public ?string $message;
    public ?string $color;

    public function __construct(
        ?int $id = null,
        ?int $brandid = null,
        ?string $message = null,
        ?string $color = null
    ) {
        $this->id = $id;
        $this->brandid = $brandid;
        $this->message = $message;
        $this->color = $color;
    }
    
}
