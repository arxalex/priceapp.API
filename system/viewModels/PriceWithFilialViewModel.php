<?php

namespace viewModels;

use framework\entities\filials\Filial;
use framework\entities\prices\PricesService;
use framework\entities\shops\ShopsService;

class PriceWithFilialViewModel
{
    private PricesService $_pricesService;
    private ShopsService $_shopsService;

    public ?string $shopLabel;
    public ?string $shopIcon;
    public ?int $shopId;
    public ?int $filialId;
    public ?string $filialCity;
    public ?string $filialStreet;
    public ?string $filialHouse;
    public ?float $xCord;
    public ?float $yCord;
    public ?int $itemId;
    public ?float $price;
    public ?float $quantity;

    public function __construct(
        int $itemId,
        Filial $filial
    ) {
        $this->_pricesService = new PricesService();
        $this->_shopsService = new ShopsService();

        $shop = $this->_shopsService->getItemFromDB($filial->shopid);
        if ($shop == null) {
            return;
        }
        $prices = $this->_pricesService->getItemsFromDB([
            'itemid' => [$itemId],
            'shopid' => [$filial->shopid],
            'filialid' => [$filial->id]
        ]);
        if (count($prices) != 1) {
            return;
        }
        $this->shopLabel = $shop->label;
        $this->shopIcon = $shop->icon;
        $this->shopId = $shop->id;
        $this->filialId = $filial->id;
        $this->filialCity = $filial->city;
        $this->filialStreet = $filial->street;
        $this->filialHouse = $filial->house;
        $this->xCord = $filial->xcord;
        $this->yCord = $filial->ycord;
        $this->itemId = $itemId;
        $price = $prices[0]->price;
        $this->price = $price->price * $price->pricefactor;
        $this->quantity = $price->quantity;
    }
}
