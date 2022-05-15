<?php

namespace businessLogic\prices;

use framework\entities\filials\FilialsService;
use viewModels\PriceWithFilialViewModel;

class PricesWebService
{
    private FilialsService $_filialsService;

    public function __construct()
    {
        $this->_filialsService = new FilialsService();
    }

    public function getPricesWithFilialsViewModelByItemIdAndCord(int $id, float $xCord, float $yCord, int $radius): array
    {
        $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
        $result = [];
        foreach ($filials as $filial) {
            $model = new PriceWithFilialViewModel($id, $filial);

            if ($model->price > 0) {
                $result[] = $model;
            }
        }
        return $result;
    }

    public function getShoppingListMultipleLowest(
        array $items,
        float $xCord,
        float $yCord,
        float $radius
    ): array {
        $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
        $result = [];
        foreach ($items as $item) {
            $item = (object) $item;
            $lowestModel = null;
            $lowestPrice = -1;
            foreach ($filials as $filial) {
                $model = new PriceWithFilialViewModel($item->itemId, $filial);

                if($lowestPrice == -1 && $model->price > 0){
                    $lowestPrice = $model->price;
                    $lowestModel = $model;
                    continue;
                }
                if ($model->price > 0 && $model->price < $lowestPrice) {
                    $lowestPrice = $model->price;
                    $lowestModel = $model;
                }
            }
            if($lowestModel != null){
                $result[] = $lowestModel;
            }
        }
        return $result;
    }
}
