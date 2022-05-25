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
        $preResult = [];
        $preResultFilials = [];
        foreach ($items as $item) {
            $item = (object) $item;
            $lowestModel = [];
            $lowestPrice = -1;
            foreach ($filials as $filial) {
                $model = new PriceWithFilialViewModel($item->itemId, $filial);

                if ($lowestPrice == -1 && $model->price > 0) {
                    $lowestModel = [];
                    $lowestPrice = $model->price;
                    $lowestModel[$model->filialId] = $model;
                    continue;
                }
                if ($model->price > 0 && $model->price < $lowestPrice) {
                    $lowestModel = [];
                    $lowestPrice = $model->price;
                    $lowestModel[$model->filialId] = $model;
                }
                if ($model->price > 0 && $model->price == $lowestPrice) {
                    $lowestModel[$model->filialId] = $model;
                }
            }
            if (count($lowestModel) > 0) {
                $preResult[] = $lowestModel;
            }
        }

        $itemIds = [];
        $filialsAfter = [];
        foreach ($preResult as $items) {
            $itemIds[] = (array_values($items)[0])->itemId;
            foreach ($items as $item) {
                if (!array_key_exists($item->filialId, $preResultFilials)) {
                    $preResultFilials[$item->filialId] = [];
                    $filialsAfter[] = $item->filialId;
                }
                $preResultFilials[$item->filialId][$item->itemId] = $item;
            }
        }

        $totalItems = count($preResult);
        $totalFilials = count($filialsAfter);
        $totalCicles = min($totalItems, $totalFilials);

        $result = [];

        for ($i = 1; $i <= $totalCicles; $i++) {
            $filialsCombination = $this->getNPairsOfFilials($i, $filialsAfter);

            foreach ($filialsCombination as $combiantion) {
                $final = [];
                $preResultCopy = $preResult;
                foreach ($combiantion as $filialId) {
                    foreach ($preResultCopy as $key => $itemsCopy) {
                        if (array_key_exists($filialId, $itemsCopy)) {
                            $final[] = $itemsCopy[$filialId];
                            unset($preResultCopy[$key]);
                        }
                    }
                }
                if (count($preResultCopy) == 0) {
                    $result = $final;
                    break 2;
                }
            }
        }

        return $result;
    }
    private function getNPairsOfFilials(int $n, array $filials): array
    {
        if ($n == 1) {
            $result = [];
            foreach ($filials as $number) {
                $result[] = [$number];
            }
            return $result;
        }

        $result = [];
        $totalFilials = count($filials);

        for ($i = 0; $i <= $totalFilials - $n; $i++) {
            $arr = $this->getNPairsOfFilials($n - 1, array_slice($filials, $i + 1));
            foreach ($arr as $value) {
                $preResult = [];
                $preResult[] = $filials[$i];
                foreach ($value as $number) {
                    $preResult[] = $number;
                }
                $result[] = $preResult;
            }
        }

        return $result;
    }
}
