<?php

namespace viewModels;

class ItemToBuyViewModel
{
    public int $itemId;
    public ?int $filialId;
    public float $count;

    public function __construct(
        int $itemId,
        ?int $filialId,
        float $count
    ) {
        $this->shopLabel = $itemId;
        $this->shopIcon = $filialId;
        $this->shopId = $count;
    }
}
