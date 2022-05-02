<?php

namespace render\main;

use render\defaultBuild\BaseRenderBuilder;

class Main extends BaseRenderBuilder
{
    public function defaultParams()
    {   
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        return '
        <div class="row">
            <div class="col-md-6 my-5 py-5">
                <h1>Цей додаток зараз розробляється</h1>
                <p>Слідкуйте за оновленнями</p>
                <div class="mb-3">
                    <a class="btn btn-success" href="http://eepurl.com/h04OAD">Підписатись на розсилку</a>
                </div>
                <div>
                    <a href="https://t.me/price_app"><i class="bi bi-telegram"></i></a>
                </div>
            </div>
            <div class="col-md-6">
                <img src="/public_resources/icons/priceapp_icon.svg">
            </div>
        </div>
        ';
    }
}
