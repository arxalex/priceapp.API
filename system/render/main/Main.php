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
        <div>
        
        </div>
        ';
    }
}
?>