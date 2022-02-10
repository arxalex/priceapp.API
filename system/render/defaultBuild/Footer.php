<?php

namespace render\defaultBuild;

use render\defaultBuild\BaseRenderBuilder;

class Footer extends BaseRenderBuilder
{
    public function defaultParams()
    {   
        return [
            'scripts' => [
                '<script src="https://code.jquery.com/jquery-3.6.0.min.js" crossorigin="anonymous"></script>',
                '<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js" crossorigin="anonymous"></script>',
                '<script src="https://cdn.jsdelivr.net/npm/vue@2.6.14/dist/vue.js"></script>',
                '<script src="https://unpkg.com/axios/dist/axios.min.js"></script>'
            ]
        ];
    }
    public function build()
    {
        $paramsString = '';
        foreach($this->getParam('scripts') as $value){
            $paramsString .= $value;
        }
        return '
                    </div>
                </div>'. 
                $paramsString .'
                <script src="/system/scripts/default/main.js"></script>
            </body>
        </html>';
    }
}
