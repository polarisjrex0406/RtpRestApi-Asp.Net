import { Tag, Divider, Row, Col, Spin, Tooltip, Dropdown } from 'antd';
import { selectMoneyFormat } from '@/redux/settings/selectors';
import { useSelector } from 'react-redux';
import useLanguage from '@/locale/useLanguage';
import { selectLangDirection } from '@/redux/translate/selectors';
import React, { useState, useRef, useEffect } from 'react';

import {
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined
} from '@ant-design/icons';

export default function Card({ data, isLoading = false, onClickViewTemplates, onClickRead, onClickEdit, onClickDelete }) {
  const translate = useLanguage();
  const [maxWidth, setMaxWidth] = useState(0);
  const divRef = useRef(null);
  const parentRef = useRef(null);

  const items = [
    {
      label: translate('Show'),
      key: 'read',
      icon: <EyeOutlined />,
    },
    {
      label: translate('Edit'),
      key: 'edit',
      icon: <EditOutlined />,
    },
    {
      type: 'divider',
    },

    {
      label: translate('Delete'),
      key: 'delete',
      icon: <DeleteOutlined />,
    },
  ];

  useEffect(() => {
    if (divRef.current && parentRef.current) {
      const handleResize = () => {
        const parentWidth = parentRef.current.getBoundingClientRect().width;
        setMaxWidth(parentWidth); // Set the max-width to 80% of the parent's width
      };

      handleResize(); // Initial calculation
      window.addEventListener('resize', handleResize);

      return () => {
        window.removeEventListener('resize', handleResize);
      };
    }
  }, [data.description, data.style, data.promptOutput]);

  return (
    <Col
      className="gutter-row"
      xs={{ span: 24 }}
      sm={{ span: 12 }}
      md={{ span: 12 }}
      lg={{ span: 12 }}
    >
      <div
        className="whiteBox shadow"
        style={{ color: '#595959', fontSize: 13, minHeight: '106px', height: '100%' }}
        onClick={() => onClickViewTemplates({ ...data })}
      >
        <div style={{ textAlign: 'right' }}>
          <Dropdown
            menu={{
              items,
              onClick: ({ key, domEvent }) => {
                domEvent.stopPropagation();
                switch (key) {
                  case 'read':
                    onClickRead({ ...data });
                    break;
                  case 'edit':
                    onClickEdit({ ...data });
                    break;
                  case 'delete':
                    onClickDelete({ ...data });
                    break;
                  default:
                    break;
                }
                // else if (key === '2')handleCloseTask
              },
            }}
            trigger={['click']}
          >
            <div
              style={{
                cursor: 'pointer', fontSize: '24px', width: '20px', float: 'right', textAlign: 'center'
              }}
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
              }}
            >⋮
            </div>
          </Dropdown>
        </div>
        <div className="pad15 strong" style={{ textAlign: 'center', justifyContent: 'center' }}>
          <h3
            style={{
              color: '#22075e',
              fontSize: 'large',
              margin: '5px 0',
              textTransform: 'capitalize',
            }}
          >
            {!isLoading ? data?.experimentCode : ''}
          </h3>
        </div>
        <Divider style={{ padding: 0, margin: 0 }}></Divider>
        <div className="pad15">
          <Row gutter={[0, 0]} justify="space-between" wrap={false} ref={parentRef}>
            <Col className="gutter-row" flex="85px" style={{ textAlign: 'left' }}>
              <div className="left" style={{ maxWidth: `${maxWidth}px`, whiteSpace: 'nowrap', textOverflow: 'ellipsis', overflow: 'hidden' }} ref={divRef}>
                Description: {!isLoading ? data?.description : ''}
              </div>
            </Col>
          </Row>
        </div>
        <div className="pad15">
          <Row gutter={[0, 0]} justify="space-between" wrap={false}>
            <Col className="gutter-row" flex="85px" style={{ textAlign: 'left' }}>
              <div className="left" style={{ maxWidth: `${maxWidth}px`, whiteSpace: 'nowrap', textOverflow: 'ellipsis', overflow: 'hidden' }} ref={divRef}>
                Style: {!isLoading ? data?.style : ''}
              </div>
            </Col>
          </Row>
        </div>
        <div className="pad15">
          <Row gutter={[0, 0]} justify="space-between" wrap={false}>
            <Col
              className="gutter-row"
              flex="auto"
              style={{
                display: 'flex',
                justifyContent: 'right',
                alignItems: 'center',
              }}
            >
              {isLoading ? (
                <Spin />
              ) : (
                <>
                  <a onClick={(e) => {
                    e.stopPropagation();
                    onClickRead({ ...data });
                  }}>
                    <EyeOutlined />
                  </a>
                  <Divider
                    style={{
                      height: '100%',
                      padding: '10px 0',
                      justifyContent: 'center',
                      alignItems: 'center',
                    }}
                    type="vertical"
                  ></Divider>
                  <a onClick={(e) => {
                    e.stopPropagation();
                    onClickEdit({ ...data });
                  }}>
                    <EditOutlined />
                  </a>
                  <Divider
                    style={{
                      height: '100%',
                      padding: '10px 0',
                      justifyContent: 'center',
                      alignItems: 'center',
                    }}
                    type="vertical"
                  ></Divider>
                  <a onClick={e => {
                    e.stopPropagation();
                    onClickDelete({ ...data });
                  }}>
                    <DeleteOutlined />
                  </a>
                </>
              )}
            </Col>
          </Row>
        </div>
      </div>
    </Col>
  );
}
